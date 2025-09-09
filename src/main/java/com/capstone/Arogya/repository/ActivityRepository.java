package com.capstone.Arogya.repository;

import com.capstone.Arogya.model.Activity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface ActivityRepository extends JpaRepository<Activity, Long> {
    List<Activity> findByGoalId(Long goalId);
}
